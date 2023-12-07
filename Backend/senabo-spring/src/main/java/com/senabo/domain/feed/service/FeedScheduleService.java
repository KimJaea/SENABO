package com.senabo.domain.feed.service;

import com.senabo.config.firebase.FCMMessage;
import com.senabo.config.firebase.FCMService;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;

@Slf4j
@Service
@RequiredArgsConstructor
public class FeedScheduleService {

    private final MemberService memberService;
    private final FeedService feedService;
    private final FCMService fcmService;

    // 1시간 마다 실행 ex) 01:00, 02:00, 03:00 ...
    @Scheduled(cron = "0 0 0/1 * * *")
    public void schedulePoop() {
        log.info("배변 스케줄러 실행");

        List<FCMMessage> feedList = memberService.findAllMemberNonComplete().stream()
                .filter(member ->member.getId() != 16)
                .map(feedService::schedulePoop)
                .filter(fcm -> fcm.getMessage().getToken() != null)
                .toList();

        fcmService.sendFCM(feedList);
    }

    // 1시간 마다 실행 ex) 01:00, 02:00, 03:00 ...
    @Scheduled(cron = "0 0 0/1 * * *")
    public void scheduleFeed() {
        log.info("배식 스케줄러 실행");

        List<FCMMessage> feedList = memberService.findAllMemberNonComplete().stream()
                .filter(member -> member.getId() != 16)
                .map(feedService::scheduleFeed)
                .filter(fcm -> fcm.getMessage().getToken() != null)
                .toList();

        fcmService.sendFCM(feedList);
    }
}
