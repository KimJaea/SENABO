package com.senabo.domain.member.service;


import com.senabo.config.firebase.FCMMessage;
import com.senabo.config.firebase.FCMService;
import com.senabo.config.security.jwt.TokenInfo;
import com.senabo.config.security.jwt.TokenProvider;
import com.senabo.domain.member.dto.request.*;
import com.senabo.domain.member.dto.response.*;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.entity.Role;
import com.senabo.domain.member.repository.MemberRepository;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.repository.ReportRepository;
import com.senabo.exception.message.ExceptionMessage;
import com.senabo.exception.model.*;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.Optional;

@Slf4j
@Service
@Transactional(readOnly = true)
@RequiredArgsConstructor
public class MemberService {

    private final MemberRepository memberRepository;
    private final ReportRepository reportRepository;
    private final RefreshTokenService refreshTokenService;
    private final TokenProvider tokenProvider;
    private final FCMService fcmService;

    public SignUpResponse signUp(SignUpRequest request) {
        Member member = memberRepository.save(
                new Member(request.dogName(), request.email(), request.species(), request.sex(), request.houseLatitude(), request.houseLongitude(), request.deviceToken()));

        reportRepository.save(
                new Report(member, 1, 0, 50)
        );
        try {
            memberRepository.flush();
            reportRepository.flush();
        } catch (DataIntegrityViolationException e) {
            throw new UserAuthException(String.valueOf(ExceptionMessage.FAIL_SAVE_DATA));
        }

        List<GrantedAuthority> roles = new ArrayList<>();
        roles.add(new SimpleGrantedAuthority(Role.ROLE_USER.toString()));

        Authentication authentication = new UsernamePasswordAuthenticationToken(member.getEmail(), null, roles);

        TokenInfo tokenInfo = tokenProvider.generateToken(authentication);

        refreshTokenService.setValues(tokenInfo.getRefreshToken(), member.getEmail());

        return SignUpResponse.from(member, tokenInfo);
    }

    @Transactional
    public void removeMember(String email, SignOutRequest request) {
        try {
            refreshTokenService.delValues(request.refreshToken());
            memberRepository.deleteByEmail(email);
        } catch (DataIntegrityViolationException e) {
            throw new UserAuthException(ExceptionMessage.FAIL_DELETE_DATA);
        }
    }

    @Transactional
    public MemberResponse getInfo(String email) {
        Member member = findByEmail(email);
        return MemberResponse.from(member);
    }

    @Transactional
    public MemberResponse updateInfo(String email, UpdateInfoRequest request) {
        Member member = findByEmail(email);
        member.update(request);
        return MemberResponse.from(member);
    }


    @Transactional
    public SignInResponse signIn(SignInRequest request) {
        Optional<Member> memberOptional = memberRepository.findByEmail(request.email());
        if (memberOptional.isEmpty()) {
            // 미 가입자
            log.info("미 가입자");
            return SignInResponse.emptyMember(false);
        }

        Member member = memberOptional.get();
        // 유효한 가입자 -> jwt 발급 및 로그인 진행
        List<GrantedAuthority> roles = new ArrayList<>();
        roles.add(new SimpleGrantedAuthority(Role.ROLE_USER.toString()));
        log.info("JWT 발급 시작");

        // jwt 발급
        Authentication authentication = new UsernamePasswordAuthenticationToken(request.email(), null, roles);
        TokenInfo tokenInfo = tokenProvider.generateToken(authentication);
        log.info("Token 발급");

        refreshTokenService.setValues(tokenInfo.getRefreshToken(), request.email());
        log.info("레디스 저장");

        return SignInResponse.from(member, tokenInfo, true);
    }

    @Transactional
    public void signOut(SignOutRequest request, String email) {
        Member member = findByEmail(email);
        member.setDeviceToken(null);
        refreshTokenService.delValues(request.refreshToken());
    }

    @Transactional
    public ReIssueResponse reissue(String refreshToken, Authentication authentication) {
        if (authentication.getName() == null) {
            throw new UserAuthException(ExceptionMessage.NOT_AUTHORIZED_ACCESS);
        }

        if (!tokenProvider.validateToken(refreshToken)) {
//            Member member = findByEmail(authentication.getName());
//            member.setDeviceToken(null);
            refreshTokenService.delValues(refreshToken);
            throw new TokenNotFoundException(ExceptionMessage.TOKEN_VALID_TIME_EXPIRED);
        }

        String email = refreshTokenService.getValues(refreshToken);
        log.info("Redis {}", email);
        log.info("Auth {}", authentication.getName());

        if (email == null || !email.equals(authentication.getName())) {
            throw new TokenCheckFailException(ExceptionMessage.MISMATCH_TOKEN);
        }

        return createAccessToken(refreshToken, authentication);
    }

    private ReIssueResponse createAccessToken(String refreshToken, Authentication authentication) {
        if (tokenProvider.checkExpiredToken(refreshToken)) {
            TokenInfo tokenInfo = tokenProvider.generateAccessToken(authentication);
            return ReIssueResponse.from(tokenInfo.getAccessToken(), "SUCCESS");
        }

        return ReIssueResponse.from(tokenProvider.generateAccessToken(authentication).getAccessToken(), "GENERAL_FAILURE");
    }

    @Transactional
    public Member findByEmail(String email) {
        return memberRepository.findByEmail(email).orElseThrow(() -> new UserException(ExceptionMessage.USER_NOT_FOUND));
    }


    public List<Member> findAllMemberNonComplete() {
        return memberRepository.findAllMemberNonComplete();
    }

    public void fcmTest() {
        log.info("FCM 테스트 시작");
        List<Member> allMember = findAllMemberNonComplete();
        List<FCMMessage> testList = allMember.stream()
                .filter(member -> member.getDeviceToken() != null)
                .map(member -> fcmService.makeMessage("세나보 테스트", "테스트입니다.", member.getDeviceToken()))
                .toList();
        fcmService.sendFCM(testList);
    }

    public void tokenCheckExcpetion() {
        throw new TokenCheckFailException(ExceptionMessage.FAIL_TOKEN_CHECK);
    }

    public void notFound() {
        throw new DataException(ExceptionMessage.DATA_NOT_FOUND);
    }

    @Transactional
    public MemberResponse updateTotalTime(String email, TotalTimeRequest request) {
        Member member = findByEmail(email);
        member.updateTotal(request);
        return MemberResponse.from(member);
    }

    public List<Member> findAllMember(){
        return memberRepository.findAll();
    }

    @Transactional
    public MemberResponse updateLocate(String email, UpdateLocateRequest request) {
        Member member = findByEmail(email);
        member.updateLocate(request);
        return MemberResponse.from(member);
    }

    @Transactional
    public MemberResponse updateDeviceToken(String email, UpdateDeviceTokenRequest request) {
        Member member = findByEmail(email);
        member.updateDeviceToken(request);
        return MemberResponse.from(member);
    }

    @Transactional
    public void updateStress(String email, int score){
        memberRepository.updateStressLevel(email, score);
    }

    @Transactional
    public void updateAffection(String email, int score){
        memberRepository.updateAffection(email, score);
    }

    public Member findById(Long id){
       return memberRepository.findById(id).orElseThrow(() -> new UserException(ExceptionMessage.USER_NOT_FOUND));
    }
}
