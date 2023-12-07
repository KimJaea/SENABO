package com.senabo.domain.feed.service;

import com.senabo.common.message.ParsingMessageService;
import com.senabo.config.firebase.FCMMessage;
import com.senabo.config.firebase.FCMService;
import com.senabo.domain.feed.dto.response.CheckFeedResponse;
import com.senabo.domain.feed.dto.response.FeedResponse;
import com.senabo.domain.feed.entity.Feed;
import com.senabo.domain.feed.repository.FeedRepository;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.stress.entity.StressType;
import com.senabo.domain.stress.service.StressService;
import com.senabo.exception.message.ExceptionMessage;
import com.senabo.exception.model.DataException;
import com.senabo.exception.model.UserException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.Duration;
import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.List;
import java.util.Optional;

@Slf4j
@Service
@Transactional(readOnly = true)
@RequiredArgsConstructor
public class FeedService {
    private final FeedRepository feedRepository;
    private final MemberService memberService;
    private final StressService stressService;
    private final ParsingMessageService parsingMessageService;
    private final FCMService fcmService;
    private final String title = "세상에 나쁜 보호자는 있다";

    @Transactional
    public FeedResponse createFeed(String email) {
        Member member = memberService.findByEmail(email);
        Feed feed = feedRepository.save(
                new Feed(member));
        try {
            feedRepository.flush();
        } catch (DataIntegrityViolationException e) {
            throw new UserException(String.valueOf(ExceptionMessage.FAIL_SAVE_DATA));
        }
        return FeedResponse.from(feed);
    }

    @Transactional
    public List<Feed> getFeed(String email) {
        Member member = memberService.findByEmail(email);
        List<Feed> feedList = feedRepository.findByMemberId(member);
        return feedList;
    }

    @Transactional
    public List<Feed> getFeedWeek(Report report, Member member) {
        LocalDateTime startTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS);
        LocalDateTime endTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS).plusDays(7);
        List<Feed> feedList = feedRepository.findFeedWeek(member, endTime, startTime);
        return feedList;
    }

    @Transactional
    public void removeFeed(String email) {
        try {
            Member member = memberService.findByEmail(email);
            List<Feed> list = feedRepository.deleteByMemberId(member);
        } catch (DataIntegrityViolationException e) {
            throw new UserException(ExceptionMessage.FAIL_DELETE_DATA);
        }
    }


    public CheckFeedResponse checkLastFeed(String email) {
        Member member = memberService.findByEmail(email);
        if (!feedRepository.existsByMemberId(member)) {
            LocalDateTime nowH = LocalDateTime.now().truncatedTo(ChronoUnit.HOURS);
            return CheckFeedResponse.from(true, nowH, nowH);
        }

        Feed feed = findLatestData(member);

        LocalDateTime now = LocalDateTime.now();
        LocalDateTime nowH = now.truncatedTo(ChronoUnit.HOURS);
        LocalDateTime lastFeedH = feed.getCreateTime().truncatedTo(ChronoUnit.HOURS);
        LocalDateTime twelveAfter = lastFeedH.plusHours(12);
        return CheckFeedResponse.from(now.isAfter(twelveAfter), lastFeedH, nowH);
    }

    public FeedResponse getFeedLatest(String email) {
        Member member = memberService.findByEmail(email);
        Feed feed = findLatestData(member);
        return FeedResponse.from(feed);
    }

    public Feed findLatestData(Member member) {
        Optional<Feed> feedOptional = feedRepository.findLatestData(member);
        if (feedOptional.isEmpty()) throw new DataException(ExceptionMessage.DATA_NOT_FOUND);
        return feedOptional.get();
    }


    @Transactional
    public FeedResponse updatePoop(String email) {
        Member member = memberService.findByEmail(email);
        Feed feed = findLatestData(member);
        if (!feed.getCleanYn()) {
            feed.update();
        }
        return FeedResponse.from(feed);
    }

    @Transactional
    public FCMMessage scheduleFeed(Member member) {
        try {
            Feed feed = findLatestData(member);
            LocalDateTime now = LocalDateTime.now();
            LocalDateTime nowH = now.truncatedTo(ChronoUnit.HOURS);
            LocalDateTime lastFeedH = feed.getCreateTime().truncatedTo(ChronoUnit.HOURS);

            LocalDateTime twelveAfter = lastFeedH.plusHours(12);
            LocalDateTime fifteenAfter = lastFeedH.plusHours(15);

            log.info("현재 시각: " + nowH);
            log.info("마지막으로 먹이준 시간: " + lastFeedH);
            log.info("마지막으로 먹이준 시간 + 12: " + twelveAfter);

            // 배식 12시간 경과
            if (nowH.equals(twelveAfter)) {
                log.info("배식 후 12시간 경과: 밥 푸시 알림 실행");
                // 밥 푸시 알림
                if (member.getDeviceToken() != null) {
                    // FCM
                    String dogName = parsingMessageService.parseLastCharacter(member.getDogName());
                    return fcmService.makeMessage(title, dogName + "의 밥을 줄 시간이에요!", member.getDeviceToken());
                }
            }
            // 배식 13시간 경과 이후 : 스트레스 1 씩 증가
            else if (nowH.isAfter(twelveAfter)) {
                log.info("배식 후 13시간 경과: 스트레스 증가");
                int originStress = member.getStressLevel();
                if (originStress == 100) return fcmService.makeEmpty();
                // 스트레스 1 증가
                Duration duration = Duration.between(nowH, lastFeedH);
                long hours = duration.toHours();
                int changeAmount = 1;
                // 배식 15시간 경과 : 스트레스 3 증가 (1회)
                if (nowH.isEqual(fifteenAfter)) {
                    log.info("배식 " + hours + "시간 경과: 공복 토 푸시 알림 및 스트레스 3 증가");
                    // 공복 토 푸시 알림
                    if (member.getDeviceToken() != null) {
                        // FCM
                        String dogName = parsingMessageService.parseLastCharacter(member.getDogName());
                        return fcmService.makeMessage(title, dogName + "가 공복이어서 토를 했어요", member.getDeviceToken());
                    }
                    // 스트레스 3 증가
                    changeAmount = 3;
                }
                stressService.saveStress(member, StressType.FEED, changeAmount);
            }
        } catch (DataException e) {
            log.error("Member ID: " + member.getId() + " 에러 발생: {}", e.getMessage());
        }

        return fcmService.makeEmpty();
    }

    @Transactional
    public FCMMessage schedulePoop(Member member) {
        try {
            Feed feed = findLatestData(member);

            // 밥 먹은 지 3시간 후인지 확인
            CheckFeedResponse response = checkLastFeed(member.getEmail());
            LocalDateTime lastFeedH = response.lastFeedDateTime();
            LocalDateTime nowH = LocalDateTime.now().truncatedTo(ChronoUnit.HOURS);

            log.info("마지막 밥 제공 시간: " + lastFeedH);

            LocalDateTime oneAfter = lastFeedH.plusHours(1);

            // 밥 먹은 지 1시간
            if (nowH.isEqual(oneAfter)) {
                // 배변 활동 푸시 알림
                if (member.getDeviceToken() != null) {
                    // FCM
                    String dogName = parsingMessageService.parseLastCharacter(member.getDogName());
                    return fcmService.makeMessage(title, dogName + "가 배변을 했어요!", member.getDeviceToken());
                }
            } else if (nowH.isAfter(oneAfter)) {
                int originStress = member.getStressLevel();
                // 밥 먹은 지 1시간 후인데 CleanYn: true || 이미 스트레스가 100
                if (feed.getCleanYn() || originStress == 100) {
                    return fcmService.makeEmpty();
                }
                // 스트레스 1 증가
                Duration duration = Duration.between(nowH, lastFeedH);
                long hours = duration.toHours();
                log.info("배변 후 " + hours + "시간 경과: 스트레스 1 증가");
                int changeAmount = 1;
                stressService.saveStress(member, StressType.POOP, changeAmount);
            }
        } catch (DataException e) {
            log.error("Member ID: " + member.getId() + " 에러 발생: {}", e.getMessage());
        }
        return fcmService.makeEmpty();
    }

}
